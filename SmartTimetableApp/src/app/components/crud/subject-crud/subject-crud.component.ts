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
  teachers: any[] = []; // For Checkboxes
  departments: any[] = []; // For Dropdown
  semesters: any[] = []; // For Dropdown

  // Matches SubjectCreateDto
  newData = { 
    name: '', 
    code: '', 
    weeklyLectures: 3, 
    isLab: false,
    semesterId: null, 
    departmentId: null, 
    teacherIds: [] as number[] 
  };

  ngOnInit() { this.loadAll(); }

  loadAll() {
    this.api.getSubjects().subscribe(d => this.subjects = d);
    this.api.getTeachers().subscribe(d => this.teachers = d); // Need these for checkboxes
    this.api.getDepartments().subscribe(d => this.departments = d);
    this.api.getSemesters().subscribe(d => this.semesters = d);
  }

  onTeacherCheck(e: any, teacherId: number) {
    if (e.target.checked) this.newData.teacherIds.push(teacherId);
    else this.newData.teacherIds = this.newData.teacherIds.filter(id => id !== teacherId);
  }

  add() {
    this.api.createSubject(this.newData).subscribe(() => {
      this.loadAll();
      // Reset Form (keep sensible defaults)
      this.newData = { name: '', code: '', weeklyLectures: 3, isLab: false, semesterId: null, departmentId: null, teacherIds: [] };
    });
  }

  delete(id: number) {
    if(confirm('Delete Subject?')) this.api.deleteSubject(id).subscribe(() => this.loadAll());
  }
}