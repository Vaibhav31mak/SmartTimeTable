import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { authGuard } from './guards/auth.guard';

// Import CRUDs
import { DepartmentCrudComponent } from './pages/setup/department-crud/department-crud.component';
import { SemesterCrudComponent } from './pages/setup/semester-crud/semester-crud.component'; 
import { RoomCrudComponent } from './pages/setup/room-crud/room-crud.component';         
import { TimeSlotCrudComponent } from './pages/setup/timeslot-crud/timeslot-crud.component'; 
import { BatchCrudComponent } from './pages/setup/batch-crud/batch-crud.component';       
import { TeacherCrudComponent } from './pages/setup/teacher-crud/teacher-crud.component'; 
import { SubjectCrudComponent } from './pages/setup/subject-crud/subject-crud.component'; 

export const routes: Routes = [
  // 1. DEFAULT ROUTE: Redirect empty path '' to 'login' immediately
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  
  // 2. PUBLIC ROUTE: The actual Login Page
  { path: 'login', component: LoginComponent },
  
  // 3. PROTECTED ROUTES: All these are behind the Guard
  { 
    path: '', 
    component: MainLayoutComponent,
    canActivate: [authGuard], // <--- This prevents access if not logged in
    children: [
      { path: 'dashboard', component: DashboardComponent },
      
      // Setup Routes
      { path: 'setup/departments', component: DepartmentCrudComponent },
      { path: 'setup/semesters', component: SemesterCrudComponent },
      { path: 'setup/rooms', component: RoomCrudComponent },
      { path: 'setup/timeslots', component: TimeSlotCrudComponent },
      { path: 'setup/batches', component: BatchCrudComponent },
      { path: 'setup/teachers', component: TeacherCrudComponent },
      { path: 'setup/subjects', component: SubjectCrudComponent },
    ]
  },

  // 4. WILDCARD: If user types junk, send to login
  { path: '**', redirectTo: 'login' }
];