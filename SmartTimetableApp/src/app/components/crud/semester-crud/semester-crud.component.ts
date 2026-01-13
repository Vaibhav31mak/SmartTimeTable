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
// ... Imports (Same as others)
export class SemesterCrudComponent implements OnInit {
  api = inject(ApiService);
  list: any[] = [];
  newData = { name: '' };

  ngOnInit() { this.load(); }
  load() { this.api.getSemesters().subscribe(d => this.list = d); }
  
  add() {
    this.api.createSemester(this.newData).subscribe(() => {
      this.load();
      this.newData = { name: '' };
    });
  }
  delete(id: number) { this.api.deleteSemester(id).subscribe(() => this.load()); }
}