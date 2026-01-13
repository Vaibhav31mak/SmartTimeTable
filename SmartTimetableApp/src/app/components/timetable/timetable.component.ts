import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import { FormsModule } from '@angular/forms'; // <--- IMPORT THIS for [(ngModel)]
import { ApiService } from '../../services/api.service';
import { TimetableEntry, TimetableGridSlot } from '../../models/timetable.model';

@Component({
  selector: 'app-timetable',
  standalone: true,
  imports: [CommonModule, FormsModule], // <--- Add FormsModule here
  templateUrl: './timetable.component.html',
  styleUrl: './timetable.component.css'
})
export class TimetableComponent implements OnInit {

  gridData: TimetableGridSlot[] = [];
  isLoading = false;
  message = '';
  
  // Define available batches (Ideally this comes from an API, but hardcoded for testing is fine)
  batches = [
    { id: 1, name: 'Semester 5 (CS-A)' },
    { id: 2, name: 'Semester 5 (CS-B)' }
  ];
  
  // Default to Batch 1
  selectedBatchId: number = 1; 

  timeSlots: string[] = [
    "09:00:00 - 10:00:00", "10:00:00 - 11:00:00", 
    "11:00:00 - 11:30:00", "11:30:00 - 12:30:00", "12:30:00 - 13:30:00"
  ];

  constructor(private api: ApiService) { }

  ngOnInit(): void { 
    this.loadTimetable(); 
  }

  // Triggered when dropdown changes
  onBatchChange() {
    this.loadTimetable();
  }

  generate() {
    this.isLoading = true;
    this.message = `Generating for Batch ${this.selectedBatchId}...`;
    
    // Pass selectedBatchId dynamically
    this.api.generateTimetable(this.selectedBatchId).subscribe({
      next: () => { 
        this.message = "Done! Reloading..."; 
        this.loadTimetable(); 
      },
      error: (err) => { 
        this.isLoading = false; 
        console.error(err); 
        this.message = "Error calling API"; 
      }
    });
  }

  loadTimetable() {
    this.isLoading = true;
    // Pass selectedBatchId dynamically
    this.api.getTimetable(this.selectedBatchId).subscribe({
      next: (data: any[]) => { 
        this.transformToGrid(data); 
        this.isLoading = false; 
      },
      error: (err) => { 
        this.isLoading = false; 
        console.error(err); 
      }
    });
  }

  transformToGrid(data: TimetableEntry[]) {
    this.gridData = this.timeSlots.map(slot => {
      const classesInSlot = data.filter(t => t.slot === slot);
      return {
        time: slot,
        monday: classesInSlot.find(d => d.day === 1),
        tuesday: classesInSlot.find(d => d.day === 2),
        wednesday: classesInSlot.find(d => d.day === 3),
        thursday: classesInSlot.find(d => d.day === 4),
        friday: classesInSlot.find(d => d.day === 5),
      };
    });
  }
}