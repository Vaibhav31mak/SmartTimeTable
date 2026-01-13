import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../services/api.service';

@Component({
  selector: 'app-semester-crud',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './semester-crud.component.html'
})
export class SemesterCrudComponent implements OnInit {
  api = inject(ApiService);
  list: any[] = [];
  
  newData = { name: '' };
  editingId: number | null = null; // 1. Track ID being edited

  ngOnInit() { this.load(); }
  load() { this.api.getSemesters().subscribe(d => this.list = d); }
  
  // 2. Load data into form
  edit(item: any) {
    this.editingId = item.id;
    this.newData = { name: item.name };
  }

  // 3. Smart Submit (Create or Update)
  submit() {
    if(!this.newData.name) return;

    if (this.editingId) {
      // UPDATE
      const payload = { id: this.editingId, ...this.newData };
      this.api.updateSemester(this.editingId, payload).subscribe(() => {
        this.load();
        this.resetForm();
      });
    } else {
      // CREATE
      this.api.createSemester(this.newData).subscribe(() => {
        this.load();
        this.resetForm();
      });
    }
  }

  resetForm() {
    this.editingId = null;
    this.newData = { name: '' };
  }

  delete(id: number) { 
    if(confirm('Delete this Semester? Warning: This might delete all associated Batches!')) {
      this.api.deleteSemester(id).subscribe(() => this.load()); 
    }
  }
}