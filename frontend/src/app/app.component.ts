import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'frontend';


  clickme(){
    fetch('__BACK1_URL__/WeatherForecast/bla').then(r => r.text()).then(v => console.log(v));
  }
}
