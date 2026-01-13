import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../services/api.service'; // Check path!

@Component({
  selector: 'app-department-crud',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './department-crud.component.html'
})
export class DepartmentCrudComponent implements OnInit {
  api = inject(ApiService);
  
  departments: any[] = [];
  newData = { name: '', code: '' }; // Matches your Backend Model

  ngOnInit() {
    this.load();
  }

  load() {
    this.api.getDepartments().subscribe(data => this.departments = data);
  }

  add() {
    if(!this.newData.name) return;
    this.api.createDepartment(this.newData).subscribe(() => {
      this.load();
      this.newData = { name: '', code: '' }; // Reset
    });
  }

  delete(id: number) {
    if(confirm('Are you sure?')) {
      // Ensure deleteDepartment exists in ApiService!
      this.api.deleteDepartment(id).subscribe(() => this.load());
    }
  }
}