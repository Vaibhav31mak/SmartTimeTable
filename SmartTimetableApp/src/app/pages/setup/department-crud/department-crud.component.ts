import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../services/api.service';

@Component({
  selector: 'app-department-crud',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './department-crud.component.html'
})
export class DepartmentCrudComponent implements OnInit {
  api = inject(ApiService);
  
  departments: any[] = [];
  
  // Model for the form
  newData = { name: '', code: '' };
  
  // Track which ID we are editing (null means Create Mode)
  editingId: number | null = null; 

  ngOnInit() {
    this.load();
  }

  load() {
    this.api.getDepartments().subscribe(data => this.departments = data);
  }

  // 1. Triggered when user clicks "Edit" on a row
  edit(item: any) {
    this.editingId = item.id;
    // Copy the data so we don't edit the table directly until saved
    this.newData = { name: item.name, code: item.code };
  }

  // 2. Triggered when user clicks "Add" or "Update"
  submit() {
    if(!this.newData.name) return;

    if (this.editingId) {
      // --- UPDATE LOGIC ---
      const payload = { id: this.editingId, ...this.newData };
      this.api.updateDepartment(this.editingId, payload).subscribe(() => {
        this.load();
        this.resetForm();
      });
    } else {
      // --- CREATE LOGIC ---
      this.api.createDepartment(this.newData).subscribe(() => {
        this.load();
        this.resetForm();
      });
    }
  }

  // 3. Helper to clean up
  resetForm() {
    this.editingId = null;
    this.newData = { name: '', code: '' };
  }

  delete(id: number) {
    if(confirm('Are you sure?')) {
      this.api.deleteDepartment(id).subscribe(() => this.load());
    }
  }
}