import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../services/api.service';

@Component({
  selector: 'app-subject-crud',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './subject-crud.component.html'
})
export class SubjectCrudComponent implements OnInit {
  api = inject(ApiService);
  subjects: any[] = [];
  teachers: any[] = [];
  departments: any[] = [];
  semesters: any[] = [];

  newData = { name: '', code: '', weeklyLectures: 3, isLab: false, semesterId: null, departmentId: null, teacherIds: [] as number[] };
  editingId: number | null = null;

  ngOnInit() { this.loadAll(); }

  loadAll() {
    this.api.getSubjects().subscribe(d => this.subjects = d);
    this.api.getTeachers().subscribe(d => this.teachers = d);
    this.api.getDepartments().subscribe(d => this.departments = d);
    this.api.getSemesters().subscribe(d => this.semesters = d);
  }

  onTeacherCheck(e: any, teacherId: number) {
    if (e.target.checked) this.newData.teacherIds.push(teacherId);
    else this.newData.teacherIds = this.newData.teacherIds.filter(id => id !== teacherId);
  }

  edit(item: any) {
    this.editingId = item.id;
    // Map backend links
    const currentTeacherIds = item.teacherSubjects ? item.teacherSubjects.map((ts: any) => ts.teacherId) : [];
    
    this.newData = { 
      name: item.name, code: item.code, weeklyLectures: item.weeklyLectures, isLab: item.isLab,
      semesterId: item.semesterId, departmentId: item.departmentId,
      teacherIds: currentTeacherIds
    };
  }

  isChecked(teacherId: number): boolean {
    return this.newData.teacherIds.includes(teacherId);
  }

  add() {
    if (this.editingId) {
      this.api.updateSubject(this.editingId, this.newData).subscribe(() => { this.loadAll(); this.resetForm(); });
    } else {
      this.api.createSubject(this.newData).subscribe(() => { this.loadAll(); this.resetForm(); });
    }
  }

  resetForm() {
    this.editingId = null;
    this.newData = { name: '', code: '', weeklyLectures: 3, isLab: false, semesterId: null, departmentId: null, teacherIds: [] };
  }

  delete(id: number) { if(confirm('Delete?')) this.api.deleteSubject(id).subscribe(() => this.loadAll()); }
}