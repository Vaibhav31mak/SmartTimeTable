import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SemesterCrudComponent } from './semester-crud.component';

describe('SemesterCrudComponent', () => {
  let component: SemesterCrudComponent;
  let fixture: ComponentFixture<SemesterCrudComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SemesterCrudComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SemesterCrudComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
