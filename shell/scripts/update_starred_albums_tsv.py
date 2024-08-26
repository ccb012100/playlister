#!/usr/bin/env python3

from colorama import Fore, Style
from enum import Enum
from pathlib import Path
from urllib.request import pathname2url
import os
import sqlite3
import sys

sql_db = str(Path.home() / "playlister.db")

# assumes it's hosted in the same repo as this script
spreadsheet = "{}/{}".format(
    os.path.abspath(os.path.dirname(__file__)), "../albums/starred_albums.tsv"
)

sql_query = """select GROUP_CONCAT(artist, '; ') as artists, album, track_count, release_date, added_at, playlist
from
(
    select art.name as artist, a.name as album, a.id as album_id, a.total_tracks as track_count, substr(a.release_date, 1, 4) as release_date, pt.added_at, p.name as playlist, p.id as playlist_id
    from Album a
    join albumartist aa on aa.album_id = a.id
    join artist art on art.id = aa.artist_id
    join track t on t.album_id = a.id
    join playlisttrack pt on pt.track_id = t.id
    join playlist p on p.id = pt.playlist_id
    where p.name like 'starred%'
    group by a.id, art.id, p.id
    order by p.id, a.id, art.name
)
group by album_id, playlist_id
order by added_at DESC
limit ? OFFSET ?"""


def get_last_album_added(spreadsheet):
    """ "
    :return: last album added in .tsv format
    """
    with open(spreadsheet, "rb") as f:
        try:  # catch OSError in case of a one line file
            f.seek(-2, os.SEEK_END)
            while f.read(1) != b"\n":
                f.seek(-2, os.SEEK_CUR)

            last_line = (
                f.readline().decode().rstrip()
            )  # trim trailing newline to be safe

            print(
                Fore.MAGENTA
                + "\nlast album added to {}:\n\n>> {}\n".format(spreadsheet, last_line)
                + Style.RESET_ALL
            )

            return last_line
        except OSError:
            f.seek(0)


def create_sqlite_connection(db_file):
    """
    :return: Connection object
    """

    try:
        # open in read-only mode; will fail if db_file doesn't exist
        return sqlite3.connect(
            "file:{}?mode=ro".format(pathname2url(db_file)), uri=True
        )
    except sqlite3.Error as e:
        print_error(e)
        sys.exit()


def row_to_tsv(row):
    """
    Returns sql row converted to .tsv format
    """
    tsv = "\t".join([str(x) for x in row])
    return tsv


def add_albums(spreadsheet, albums):
    if albums:
        data_to_write = "\n" + "\n".join(albums)
        print_info("adding to  " + spreadsheet + ":\n---" + data_to_write + "\n")

        with open(spreadsheet, "a") as f:  # open in append mode
            f.write(data_to_write)

        print_success("\n✔️ Success: added {} albums! ✔️".format(str(len(albums))))
    else:
        print_error("Error: entries collection was empty")
        sys.exit()


Message_Level = Enum("Message_Level", ["ERROR", "SUCCESS", "WARNING", "INFO"])


def print_error(message):
    print_message(Message_Level.ERROR, "ERROR: " + message)


def print_success(message):
    print_message(Message_Level.SUCCESS, message)


def print_warning(message):
    print_message(Message_Level.WARNING, "WARNING: " + message)


def print_info(message):
    print_message(Message_Level.INFO, message)


def print_message(message_level, message):
    message_level_colors = {
        Message_Level.ERROR: Fore.RED + Style.BRIGHT,
        Message_Level.SUCCESS: Fore.GREEN + Style.BRIGHT,
        Message_Level.WARNING: Fore.YELLOW,
        Message_Level.INFO: Fore.BLUE,
    }

    print(message_level_colors.get(message_level, "") + message + Style.RESET_ALL)


last_added = get_last_album_added(spreadsheet)

sql_conn = create_sqlite_connection(sql_db)
sql_cur = sql_conn.cursor()

limit = 50
offset = 0

sql_cur.execute(sql_query, [limit, offset])
db_row = sql_cur.fetchall()

new_entries = []

# TODO: increment offset and retrieve more entries if no matches are found
for row in db_row:
    album = row_to_tsv(row)

    if album == last_added:
        if new_entries:
            # reverse the list to order them in ASC order
            new_entries.reverse()
            add_albums(spreadsheet, new_entries)
        else:
            print_success(
                "\n📢 Nothing to add; the spreadsheet is already up to date! 📢"
            )
        sys.exit()
    else:
        new_entries.append(album)

if sql_conn:
    sql_conn.close()
else:
    print_warning("connection was already closed")

print_error("was unable to find the last entry in the database")
