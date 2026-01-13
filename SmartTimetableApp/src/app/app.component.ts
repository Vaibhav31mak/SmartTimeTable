import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TimetableComponent } from './components/timetable/timetable.component'; // <--- Import Child]

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ RouterOutlet], // <--- Add to Imports array
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'SmartTimetableApp';
}