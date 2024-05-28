import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SearchProductCardComponent } from './search-product-card.component';

describe('SearchProductCardComponent', () => {
  let component: SearchProductCardComponent;
  let fixture: ComponentFixture<SearchProductCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SearchProductCardComponent],
    }).compileComponents();
    fixture = TestBed.createComponent(SearchProductCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});