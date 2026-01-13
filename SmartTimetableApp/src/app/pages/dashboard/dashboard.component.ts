import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import { FormsModule } from '@angular/forms'; 
import { ApiService } from '../../services/api.service';
import { TimetableEntry, TimetableGridSlot } from '../../models/timetable.model';

// 1. IMPORT THESE TWO
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule], 
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  private api = inject(ApiService);

  gridData: TimetableGridSlot[] = [];
  isLoading = false;
  message = '';
  
  batches: any[] = []; 
  selectedBatchId: number = 0; 

  timeSlots: string[] = [
    "09:00:00 - 10:00:00", "10:00:00 - 11:00:00", 
    "11:00:00 - 11:30:00", "11:30:00 - 12:30:00", "12:30:00 - 13:30:00"
  ];

  ngOnInit(): void { 
    this.loadBatches();
  }

  loadBatches() {
    this.api.getBatches().subscribe({
      next: (data) => {
        this.batches = data;
        if(this.batches.length > 0) {
            this.selectedBatchId = this.batches[0].id;
            this.loadTimetable();
        }
      },
      error: () => {
         this.message = "Could not load batches. Is the backend running?";
      }
    });
  }

  onBatchChange() {
    this.loadTimetable();
  }

  generate() {
    this.isLoading = true;
    this.message = `Generating...`;
    
    this.api.generateTimetable(this.selectedBatchId).subscribe({
      next: () => { 
        this.message = "Done! Timetable generated."; 
        this.loadTimetable(); 
      },
      error: (err) => { 
        this.isLoading = false; 
        this.message = "Error: " + (err.error?.message || "Generation failed"); 
      }
    });
  }

  loadTimetable() {
    if(!this.selectedBatchId) return;

    this.isLoading = true;
    this.api.getTimetable(this.selectedBatchId).subscribe({
      next: (data: TimetableEntry[]) => { 
        this.transformToGrid(data); 
        this.isLoading = false; 
        this.message = '';
      },
      error: () => { 
        this.isLoading = false; 
        this.message = "No timetable found for this batch.";
      }
    });
  }

  transformToGrid(data: TimetableEntry[]) {
    this.gridData = this.timeSlots.map(slot => {
      const classesInSlot = data.filter(t => t.slot.trim() === slot.trim());
      
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

  // ============================================
  // 2. ADD THIS FUNCTION TO GENERATE PDF
  // ============================================
  public downloadPDF(): void {
    const DATA = document.getElementById('timetable-print'); // Matches the ID in HTML

    if(DATA) {
      html2canvas(DATA).then(canvas => {
        const fileWidth = 297; // A4 Width in mm (Landscape)
        const fileHeight = (canvas.height * fileWidth) / canvas.width;

        const FILEURI = canvas.toDataURL('image/png');
        const PDF = new jsPDF('l', 'mm', 'a4'); // 'l' means Landscape orientation
        
        const position = 0;
        PDF.addImage(FILEURI, 'PNG', 0, position, fileWidth, fileHeight);
        
        // Save with dynamic name (e.g., "Timetable_CS-A.pdf")
        const batchName = this.batches.find(b => b.id == this.selectedBatchId)?.name || 'Batch';
        PDF.save(`Timetable_${batchName}.pdf`);
      });
    }
  }
}