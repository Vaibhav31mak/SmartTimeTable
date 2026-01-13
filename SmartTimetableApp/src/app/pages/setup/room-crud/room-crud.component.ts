import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../services/api.service';

@Component({
  selector: 'app-room-crud',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './room-crud.component.html'
})
export class RoomCrudComponent implements OnInit {
  api = inject(ApiService);
  list: any[] = [];
  
  // Form Model
  newData = { name: '', capacity: 60, isLab: false };
  
  // Track Edit Mode
  editingId: number | null = null;

  ngOnInit() { this.load(); }
  load() { this.api.getRooms().subscribe(d => this.list = d); }

  // 1. Enter Edit Mode
  edit(item: any) {
    this.editingId = item.id;
    // Copy all properties so we don't mutate the list directly
    this.newData = { 
      name: item.name, 
      capacity: item.capacity, 
      isLab: item.isLab 
    };
  }

  // 2. Smart Submit
  submit() {
    if(!this.newData.name) return;

    if (this.editingId) {
      // UPDATE: Important! Include the ID in the payload
      const payload = { id: this.editingId, ...this.newData };
      
      this.api.updateRoom(this.editingId, payload).subscribe(() => {
        this.load();
        this.resetForm();
      });
    } else {
      // CREATE
      this.api.createRoom(this.newData).subscribe(() => {
        this.load();
        this.resetForm(); // Reset keeps default capacity=60
      });
    }
  }

  resetForm() {
    this.editingId = null;
    this.newData = { name: '', capacity: 60, isLab: false };
  }

  delete(id: number) { 
    if(confirm('Delete this room?')) {
      this.api.deleteRoom(id).subscribe(() => this.load()); 
    }
  }
}