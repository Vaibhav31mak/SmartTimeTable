import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TimetableEntry } from '../models/timetable.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5192/api'; // Check your port!

  // ==========================
  // 1. DASHBOARD / TIMETABLE
  // ==========================
  getBatches(): Observable<any[]> { return this.http.get<any[]>(`${this.baseUrl}/Batches`); }
  getTimetable(batchId: number): Observable<TimetableEntry[]> { return this.http.get<TimetableEntry[]>(`${this.baseUrl}/Timetable/${batchId}`); }
  generateTimetable(batchId: number): Observable<any> { return this.http.post(`${this.baseUrl}/Timetable/generate/${batchId}`, {}); }

  // ==========================
  // 2. DROPDOWN HELPERS (GET)
  // ==========================
  getDepartments(): Observable<any[]> { return this.http.get<any[]>(`${this.baseUrl}/Departments`); }
  getSemesters(): Observable<any[]> { return this.http.get<any[]>(`${this.baseUrl}/Semesters`); }
  getRooms(): Observable<any[]> { return this.http.get<any[]>(`${this.baseUrl}/Rooms`); }
  getTimeSlots(): Observable<any[]> { return this.http.get<any[]>(`${this.baseUrl}/TimeSlots`); }
  getTeachers(): Observable<any[]> { return this.http.get<any[]>(`${this.baseUrl}/Teachers`); }
  getSubjects(): Observable<any[]> { return this.http.get<any[]>(`${this.baseUrl}/Subjects`); }

  // ==========================
  // 3. CREATE METHODS (POST)
  // ==========================
  createDepartment(data: any) { return this.http.post(`${this.baseUrl}/Departments`, data); }
  createSemester(data: any) { return this.http.post(`${this.baseUrl}/Semesters`, data); }
  createRoom(data: any) { return this.http.post(`${this.baseUrl}/Rooms`, data); }
  createTimeSlot(data: any) { return this.http.post(`${this.baseUrl}/TimeSlots`, data); }
  createBatch(data: any) { return this.http.post(`${this.baseUrl}/Batches`, data); }
  
  // Complex Creates (with Checkbox IDs)
  createTeacher(data: any) { return this.http.post(`${this.baseUrl}/Teachers`, data); }
  createSubject(data: any) { return this.http.post(`${this.baseUrl}/Subjects`, data); }

  // ==========================
  // 4. DELETE METHODS (DELETE)
  // ==========================
  deleteDepartment(id: number) { return this.http.delete(`${this.baseUrl}/Departments/${id}`); }
  deleteSemester(id: number) { return this.http.delete(`${this.baseUrl}/Semesters/${id}`); }
  deleteRoom(id: number) { return this.http.delete(`${this.baseUrl}/Rooms/${id}`); }
  deleteTimeSlot(id: number) { return this.http.delete(`${this.baseUrl}/TimeSlots/${id}`); }
  deleteBatch(id: number) { return this.http.delete(`${this.baseUrl}/Batches/${id}`); }
  deleteTeacher(id: number) { return this.http.delete(`${this.baseUrl}/Teachers/${id}`); }
  deleteSubject(id: number) { return this.http.delete(`${this.baseUrl}/Subjects/${id}`); }


  // Add this method inside your ApiService class
updateDepartment(id: number, data: any) {
  return this.http.put(`${this.baseUrl}/Departments/${id}`, data);
}
updateSemester(id: number, data: any) {
  return this.http.put(`${this.baseUrl}/Semesters/${id}`, data);
}
updateRoom(id: number, data: any) {
  return this.http.put(`${this.baseUrl}/Rooms/${id}`, data);
}
updateTimeSlot(id: number, data: any) {
  return this.http.put(`${this.baseUrl}/TimeSlots/${id}`, data);
}
// ... existing update methods ...
updateBatch(id: number, data: any) {
  return this.http.put(`${this.baseUrl}/Batches/${id}`, data);
}

// For Teachers & Subjects, we send the DTO (with the list of IDs)
updateTeacher(id: number, data: any) {
  return this.http.put(`${this.baseUrl}/Teachers/${id}`, data);
}

updateSubject(id: number, data: any) {
  return this.http.put(`${this.baseUrl}/Subjects/${id}`, data);
}
}
