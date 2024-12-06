import { Component, OnInit } from '@angular/core';
import { ExchangeRateService } from '../../services/exchange-rate-service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-exchange-rate-list',
  templateUrl: './exchange-rate-list.component.html',
  standalone: true,
  imports: [CommonModule],
  styleUrls: ['./exchange-rate-list.component.css'],
})
export class ExchangeRateListComponent implements OnInit {
  exchangeRates: any[] = [];
  baseRates: Record<string, number> = {};
  tolerance: number = 0.001;

  constructor(private exchangeRateService: ExchangeRateService) {}

  ngOnInit(): void {
    this.loadBaseRates();
    setInterval(() => this.loadExchangeRates(), 20000);
  }

  loadBaseRates() {
    this.exchangeRateService.getExchangeRates().subscribe((data: any) => {
      this.baseRates = { ...data.rates };
      this.exchangeRates = this.calculateChange(data.rates);
    });
  }

  loadExchangeRates() {
    this.exchangeRateService.getExchangeRates().subscribe((data: any) => {
      this.exchangeRates = this.calculateChange(data.rates);
    });
  }

  calculateChange(currentRates: Record<string, number>) {
    return Object.keys(currentRates).map((currency) => {
      const baseValue = this.baseRates[currency];
      const currentValue = currentRates[currency];
      const change =
        baseValue !== undefined && baseValue !== 0
          ? ((currentValue - baseValue) / baseValue) * 100
          : 0;

      return {
        currency,
        value: currentValue,
        change: parseFloat(change.toFixed(8)),
      };
    });
  }

  onImageError(event: Event) {
    const target = event.target as HTMLImageElement;
    target.src = 'unknown-flag.png';
  }  
}
