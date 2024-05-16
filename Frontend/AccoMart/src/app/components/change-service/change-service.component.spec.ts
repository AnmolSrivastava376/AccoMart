import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeServiceComponent } from './change-service.component';

describe('ChangeServiceComponent', () => {
  let component: ChangeServiceComponent;
  let fixture: ComponentFixture<ChangeServiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChangeServiceComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ChangeServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
