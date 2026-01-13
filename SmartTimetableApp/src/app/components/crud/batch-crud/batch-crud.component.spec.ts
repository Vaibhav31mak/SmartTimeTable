import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BatchCrudComponent } from './batch-crud.component';

describe('BatchCrudComponent', () => {
  let component: BatchCrudComponent;
  let fixture: ComponentFixture<BatchCrudComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BatchCrudComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BatchCrudComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
