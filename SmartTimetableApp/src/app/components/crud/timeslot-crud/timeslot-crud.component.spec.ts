import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TimeslotCrudComponent } from './timeslot-crud.component';

describe('TimeslotCrudComponent', () => {
  let component: TimeslotCrudComponent;
  let fixture: ComponentFixture<TimeslotCrudComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TimeslotCrudComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TimeslotCrudComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
