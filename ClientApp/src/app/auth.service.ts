import {Inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {DOCUMENT} from '@angular/common';
import {catchError, tap} from 'rxjs/operators';
import {Observable, of} from "rxjs";

import {environment} from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  static apiUrl: string = environment.apiUrl;
  private authStartUrl: string = `${AuthService.apiUrl}/start/auth`;

  constructor(@Inject(DOCUMENT) private document: Document, private http: HttpClient) {
  }

  login() {
    this.getAuthUrl().subscribe(authUrl => document.location.href = authUrl);
  }

  private getAuthUrl(): Observable<string> {
    return this.http.get<string>(this.authStartUrl).pipe(
      tap(_ => console.log(`Called ${this.authStartUrl}`)),
      catchError(this.handleError<string>('getAuthUrl'))
    );
  }

  /**
   * Handle Http operation that failed.
   * Let the app continue.
   * @param operation - name of the operation that failed
   * @param result - optional value to return as the observable result
   */
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console

      // TODO: better job of transforming error for user consumption
      console.log(`${operation} failed: ${error.message}`);

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }
}
