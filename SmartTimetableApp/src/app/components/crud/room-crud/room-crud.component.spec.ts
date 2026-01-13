import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomCrudComponent } from './room-crud.component';

describe('RoomCrudComponent', () => {
  let component: RoomCrudComponent;
  let fixture: ComponentFixture<RoomCrudComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoomCrudComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RoomCrudComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
