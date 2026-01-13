import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TimetableService {
  private http = inject(HttpClient);
  
  // Matches your .NET HTTP port
  private apiUrl = 'http://localhost:5192/api/Timetable'; 

  // GET: Fetch a specific timetable (e.g., Semester 1)
  getTimetable(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  // POST: Trigger generation
  generateTimetable(id: number): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/generate/${id}`, {});
  }
}