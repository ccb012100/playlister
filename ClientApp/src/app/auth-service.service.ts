import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthServiceService {
  static apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) {
  }
}
