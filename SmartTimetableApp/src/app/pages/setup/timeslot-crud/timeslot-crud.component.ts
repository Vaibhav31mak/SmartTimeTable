import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../services/api.service';

@Component({
  selector: 'app-timeslot-crud',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './timeslot-crud.component.html'
})
export class TimeSlotCrudComponent implements OnInit {
  api = inject(ApiService);
  list: any[] = [];
  
  // Form Model
  newData = { startTime: '', endTime: '', isLunchBreak: false };
  
  // Track Edit Mode
  editingId: number | null = null;

  ngOnInit() { this.load(); }
  load() { this.api.getTimeSlots().subscribe(d => this.list = d); }

  // 1. Enter Edit Mode
  edit(item: any) {
    this.editingId = item.id;
    
    // FORMAT FIX: Backend sends "09:00:00", but Input needs "09:00"
    // We slice the first 5 characters.
    this.newData = { 
      startTime: item.startTime.toString().substring(0, 5), 
      endTime: item.endTime.toString().substring(0, 5), 
      isLunchBreak: item.isLunchBreak 
    };
  }

  // 2. Smart Submit
  submit() {
    if(!this.newData.startTime || !this.newData.endTime) return;

    // Prepare Payload (Append seconds for Backend)
    const payload = { 
      id: this.editingId, // Included for safety
      startTime: this.newData.startTime.length === 5 ? this.newData.startTime + ":00" : this.newData.startTime,
      endTime: this.newData.endTime.length === 5 ? this.newData.endTime + ":00" : this.newData.endTime,
      isLunchBreak: this.newData.isLunchBreak
    };

    if (this.editingId) {
      // UPDATE
      this.api.updateTimeSlot(this.editingId, payload).subscribe(() => {
        this.load();
        this.resetForm();
      });
    } else {
      // CREATE
      this.api.createTimeSlot(payload).subscribe(() => {
        this.load();
        this.resetForm();
      });
    }
  }

  resetForm() {
    this.editingId = null;
    this.newData = { startTime: '', endTime: '', isLunchBreak: false };
  }

  delete(id: number) { 
    if(confirm('Delete this Time Slot?')) {
      this.api.deleteTimeSlot(id).subscribe(() => this.load()); 
    }
  }
}