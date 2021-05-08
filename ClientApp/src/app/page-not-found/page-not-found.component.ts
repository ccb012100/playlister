import {Component, OnInit} from '@angular/core';
import {NavigationEnd, Router, RouterEvent} from '@angular/router';
import {filter} from 'rxjs/operators';

@Component({
  selector: 'app-page-not-found',
  templateUrl: './page-not-found.component.html',
  styleUrls: ['./page-not-found.component.sass']
})
export class PageNotFoundComponent implements OnInit {
  ngOnInit(): void {
  }

  currentRoute: string;

  constructor(private router: Router) {
    router.events.pipe(
      filter(event => event instanceof NavigationEnd)).subscribe(
      event => {
        if (event instanceof RouterEvent) {
          this.currentRoute = event?.url;
        } else throw Error(`Expected RouterEvent type but got "${typeof (event)}"`)
      });
  }
}
