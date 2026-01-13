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
  newData = { name: '', capacity: 60, isLab: false };

  ngOnInit() { this.load(); }
  load() { this.api.getRooms().subscribe(d => this.list = d); }

  add() {
    this.api.createRoom(this.newData).subscribe(() => {
      this.load();
      this.newData = { name: '', capacity: 60, isLab: false };
    });
  }
  delete(id: number) { this.api.deleteRoom(id).subscribe(() => this.load()); }
}