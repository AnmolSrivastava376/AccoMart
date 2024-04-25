import { Component } from '@angular/core';
import { CategoryNavbarComponent } from "../../components/category-navbar/category-navbar.component";

@Component({
    selector: 'app-home',
    standalone: true,
    templateUrl: './home.component.html',
    styleUrl: './home.component.css',
    imports: [CategoryNavbarComponent]
})
export class HomeComponent {
    categories: string[] = ['Electronics', 'Footwear', 'Clothing', 'Drinks'];
}
