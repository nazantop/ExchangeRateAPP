import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ExchangeRateListComponent } from './exchange-rate-list/exchange-rate-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, ExchangeRateListComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'currency-exchange';
}
