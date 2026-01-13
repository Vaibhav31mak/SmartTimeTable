import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../services/api.service';

@Component({
  selector: 'app-batch-crud',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './batch-crud.component.html'
})
// ... imports
export class BatchCrudComponent implements OnInit {
  api = inject(ApiService);
  batches: any[] = [];
  semesters: any[] = [];
  departments: any[] = [];

  newData = { name: '', semesterId: null, departmentId: null, capacity: 60 };
  editingId: number | null = null; // Track Edit

  ngOnInit() { this.loadAll(); }

  loadAll() {
    this.api.getBatches().subscribe(d => this.batches = d);
    this.api.getSemesters().subscribe(d => this.semesters = d);
    this.api.getDepartments().subscribe(d => this.departments = d);
  }

  edit(item: any) {
    this.editingId = item.id;
    this.newData = { 
      name: item.name, 
      capacity: item.capacity,
      semesterId: item.semesterId, 
      departmentId: item.departmentId 
    };
  }

  add() { // Rename this to submit() in your head, or keep as add() but change logic
    if (this.editingId) {
       // UPDATE
       const payload = { id: this.editingId, ...this.newData };
       this.api.updateBatch(this.editingId, payload).subscribe(() => {
         this.loadAll();
         this.resetForm();
       });
    } else {
       // CREATE
       this.api.createBatch(this.newData).subscribe(() => {
         this.loadAll();
         this.resetForm();
       });
    }
  }

  resetForm() {
    this.editingId = null;
    this.newData = { name: '', semesterId: null, departmentId: null, capacity: 60 };
  }

  delete(id: number) {
    if(confirm('Delete batch?')) this.api.deleteBatch(id).subscribe(() => this.loadAll());
  }

  getSemesterName(id: number) { return this.semesters.find(s => s.id === id)?.name || id; }
  getDeptName(id: number) { return this.departments.find(d => d.id === id)?.name || id; }
}