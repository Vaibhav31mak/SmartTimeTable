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
export class BatchCrudComponent implements OnInit {
  api = inject(ApiService);
  
  batches: any[] = [];
  semesters: any[] = [];
  departments: any[] = [];

  // Matches Backend: { Name, SemesterId, DepartmentId, Capacity }
  newData = { name: '', semesterId: null, departmentId: null, capacity: 60 };

  ngOnInit() {
    this.loadAll();
  }

  loadAll() {
    // Parallel fetching
    this.api.getBatches().subscribe(d => this.batches = d);
    this.api.getSemesters().subscribe(d => this.semesters = d);
    this.api.getDepartments().subscribe(d => this.departments = d);
  }

  add() {
    this.api.createBatch(this.newData).subscribe(() => {
      this.loadAll();
      this.newData = { name: '', semesterId: null, departmentId: null, capacity: 60 };
    });
  }

  delete(id: number) {
    if(confirm('Delete this batch?')) {
        this.api.deleteBatch(id).subscribe(() => this.loadAll());
    }
  }

  // Helpers to display Names instead of IDs in the table
  getSemesterName(id: number) { return this.semesters.find(s => s.id === id)?.name || id; }
  getDeptName(id: number) { return this.departments.find(d => d.id === id)?.name || id; }
}