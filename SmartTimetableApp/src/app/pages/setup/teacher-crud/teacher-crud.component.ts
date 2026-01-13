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
  subjects: any[] = [];

  newData = { name: '', departmentId: null, subjectIds: [] as number[] };
  editingId: number | null = null;

  ngOnInit() { this.loadAll(); }

  loadAll() {
    this.api.getTeachers().subscribe(d => this.teachers = d);
    this.api.getDepartments().subscribe(d => this.departments = d);
    this.api.getSubjects().subscribe(d => this.subjects = d);
  }

  onSubjectCheck(e: any, subjectId: number) {
    if (e.target.checked) this.newData.subjectIds.push(subjectId);
    else this.newData.subjectIds = this.newData.subjectIds.filter(id => id !== subjectId);
  }

  edit(item: any) {
    this.editingId = item.id;
    // EXTRACT IDs from the manually joined list
    const currentSubjectIds = item.teacherSubjects ? item.teacherSubjects.map((ts: any) => ts.subjectId) : [];

    this.newData = { 
      name: item.name, 
      departmentId: item.departmentId,
      subjectIds: currentSubjectIds
    };
  }

  isChecked(subjectId: number): boolean {
    return this.newData.subjectIds.includes(subjectId);
  }

  add() {
    if (this.editingId) {
       this.api.updateTeacher(this.editingId, this.newData).subscribe(() => {
         this.loadAll();
         this.resetForm();
       });
    } else {
       this.api.createTeacher(this.newData).subscribe(() => {
         this.loadAll();
         this.resetForm();
       });
    }
  }

  resetForm() {
    this.editingId = null;
    this.newData = { name: '', departmentId: null, subjectIds: [] };
  }

  delete(id: number) { if(confirm('Delete?')) this.api.deleteTeacher(id).subscribe(() => this.loadAll()); }
}