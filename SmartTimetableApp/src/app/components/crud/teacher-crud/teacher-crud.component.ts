import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../services/api.service';

@Component({
  selector: 'app-teacher-crud',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './teacher-crud.component.html'
})
export class TeacherCrudComponent implements OnInit {
  api = inject(ApiService);
  
  teachers: any[] = [];
  departments: any[] = [];
  subjects: any[] = []; // Used for checkboxes

  // Model matches TeacherCreateDto
  newData = { 
    name: '', 
    departmentId: null, 
    subjectIds: [] as number[] 
  };

  ngOnInit() {
    this.loadAll();
  }

  loadAll() {
    this.api.getTeachers().subscribe(d => this.teachers = d);
    this.api.getDepartments().subscribe(d => this.departments = d);
    this.api.getSubjects().subscribe(d => this.subjects = d);
  }

  // ⚡ CHECKBOX LOGIC
  onSubjectCheck(e: any, subjectId: number) {
    if (e.target.checked) {
      this.newData.subjectIds.push(subjectId);
    } else {
      this.newData.subjectIds = this.newData.subjectIds.filter(id => id !== subjectId);
    }
  }

  add() {
    this.api.createTeacher(this.newData).subscribe(() => {
      this.loadAll();
      this.newData = { name: '', departmentId: null, subjectIds: [] }; // Reset
    });
  }

  delete(id: number) {
    if(confirm('Delete teacher?')) {
        this.api.deleteTeacher(id).subscribe(() => this.loadAll());
    }
  }
}