import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'frontend';


  clickme(){
    fetch('http://localhost:8080/WeatherForecast/bla').then(r => r.text()).then(v => console.log(v));
  }
}
