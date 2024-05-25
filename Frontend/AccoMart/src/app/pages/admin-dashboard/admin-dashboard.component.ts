import { Component, OnInit } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { Chart, ChartModule } from 'angular-highcharts';
import { ChartService } from '../../services/chart.service';
import { ChartCategoryItem } from '../../interfaces/chartCategoryItem';
import {  HttpClientModule } from '@angular/common/http';
import { ChartOrderItem } from '../../interfaces/chartOrderItem';
import { ChartProductItem } from '../../interfaces/chartProductItem';


@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [NavbarComponent, SidebarComponent,ChartModule,HttpClientModule],
  providers:[ChartService],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.css'
})
export class AdminDashboardComponent implements  OnInit {
  pieChart: Chart; 
  lineChart:Chart;
  pieData:ChartCategoryItem[];
  lineData:ChartOrderItem[];
  barData:ChartProductItem[];
  barChart: Chart; 

  constructor(private chartService:ChartService){}
  ngOnInit(): void {

    this.chartService.fetchCategoryWiseQuantity().subscribe(data => {
      this.pieData = data;
      this.initPieChart();  
  })

    this.chartService.fetchDailyOrderQuantity().subscribe(data=>{
      this.lineData = data;
      this.initLineChart();
    })

    this.chartService.fetchProductWiseQuantity().subscribe(data=>{
      this.barData = data.sort((a, b) => b.quantity - a.quantity).slice(0,7);
      this.initBarChart();
      
    })

  }
  
  initPieChart(): void {
    const categories = this.pieData.map(item => item.categoryName);
    const quantities = this.pieData.map(item => item.quantity);

    this.pieChart = new Chart({
      chart: {
        type: 'pie',
        plotShadow: false,
      },
      credits: {
        enabled: false,
      },
      plotOptions: {
        pie: {
          innerSize: '70',
          borderWidth: 12,
          borderColor: '',
          slicedOffset: 10,
          dataLabels: {
            connectorWidth: 0,
            style: { // Add the style property here
              fontSize: '10px' // Adjust the font size as per your requirement
            }
          },
  
        },
      },
      title: {
      
        text: 'Category wise sales',
        style:{
        fontSize:'8px;',
        
        }
      },
      legend: {
        enabled: true,
      },
      series: [{
        type: 'pie',
        data: this.pieData.map(item => ({
          name: item.categoryName,
          y: item.quantity,
          color: this.getRandomColor() // Get a random color for each category
        }))
      }]
    });
  }

  getRandomColor(): string {
    const letters = '0123456789ABCDEF';
    let color = '#';
    for (let i = 0; i < 6; i++) {
      color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
  }

  initLineChart(): void {
    const dates = this.lineData.map(entry => new Date(entry.orderDate).toISOString().split('T')[0]);
    const sales = this.lineData.map(entry => entry.totalSales);

    this.lineChart = new Chart({
      chart: {
        type: 'line'
      },

      title: {
        text: 'Day wise Sales',
        style:{
          color:''
        }
      },
      xAxis: {
        categories: dates
      
      },
      yAxis: {
        title: {
          text: 'Total Sales in ₹'
        }
      },
      series: [{
        type:'line',
        name: 'Total Sales',
        data: sales
      }]

    });
  }
  initBarChart(): void {
    // Extract product names and quantities from the fetched data
    const products = this.barData.map(item => item.productName);
    const quantities = this.barData.map(item => item.quantity);

    // Create the bar chart
    this.barChart = new Chart({
      chart: {
        type: 'bar'
      },
      title: {
        text: 'Top selling Products'
      },
      xAxis: {
        categories: products
      },
      yAxis: {
        title: {
        }
      },
      series: [{
        type:'bar',
        name: 'Quantity',
        data: quantities
      }]
    });
  }


}

  


