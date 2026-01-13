// ... Imports
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
  // Use strings for time inputs
  newData = { startTime: '', endTime: '', isLunchBreak: false };

  ngOnInit() { this.load(); }
  load() { this.api.getTimeSlots().subscribe(d => this.list = d); }

  add() {
    // Append seconds to satisfy TimeSpan if needed, or rely on DTO parsing
    const payload = { ...this.newData };
    if(payload.startTime.length === 5) payload.startTime += ":00";
    if(payload.endTime.length === 5) payload.endTime += ":00";

    this.api.createTimeSlot(payload).subscribe(() => {
      this.load();
      this.newData = { startTime: '', endTime: '', isLunchBreak: false };
    });
  }
  delete(id: number) { this.api.deleteTimeSlot(id).subscribe(() => this.load()); }
}