import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GridDisplayCardComponent } from './grid-display-card.component';

describe('GridDisplayCardComponent', () => {
  let component: GridDisplayCardComponent;
  let fixture: ComponentFixture<GridDisplayCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GridDisplayCardComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(GridDisplayCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
