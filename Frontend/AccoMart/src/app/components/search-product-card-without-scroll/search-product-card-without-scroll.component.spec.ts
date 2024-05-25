import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchProductCardWithoutScrollComponent } from './search-product-card-without-scroll.component';

describe('SearchProductCardWithoutScrollComponent', () => {
  let component: SearchProductCardWithoutScrollComponent;
  let fixture: ComponentFixture<SearchProductCardWithoutScrollComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SearchProductCardWithoutScrollComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SearchProductCardWithoutScrollComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
