import { Component, EventEmitter, Output } from '@angular/core';
import { NavbarComponent } from '../navbar/navbar.component';
import { FeatureCardComponent } from '../feature-card/feature-card.component';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IFilterForm, ISelectedFilters } from '../interface/feature.interface';
import { FeatureService } from '../services/feature.service';


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [NavbarComponent, FeatureCardComponent, FormsModule, ReactiveFormsModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {

  selectedFiltersForm = new FormGroup<IFilterForm>({
    searchQuery: new FormControl<string | null>(null),
    featureToggleFilter: new FormControl<boolean | null>(null),
    releaseToggleFilter: new FormControl<boolean | null>(null),
    enabledFilter: new FormControl<boolean | null>(null),
    disabledFilter: new FormControl<boolean | null>(null),
  });

  selectedFiltersFormValue : ISelectedFilters = this.selectedFiltersForm.value; 

  @Output() applyFiltersEvent2 = new EventEmitter<ISelectedFilters>(); 

  constructor(private featureService: FeatureService) { }

  ngOnInit(){
    this.selectedFiltersForm.get('enabledFilter')?.disable();
    this.selectedFiltersForm.get('disabledFilter')?.disable();
  } 

  applyFilters(): void {
    this.selectedFiltersFormValue = this.selectedFiltersForm.value;
    this.applyFiltersEvent2.emit(this.selectedFiltersFormValue);
  }

  rtOptionsCheck(){
    if(this.selectedFiltersForm.value.releaseToggleFilter === false 
      ||this.selectedFiltersForm.value.releaseToggleFilter === null 
    ){
      this.selectedFiltersForm.get('featureToggleFilter')?.enable();
      this.selectedFiltersForm.get('enabledFilter')?.setValue(null);
      this.selectedFiltersForm.get('enabledFilter')?.disable();
      this.selectedFiltersForm.get('disabledFilter')?.setValue(null);
      this.selectedFiltersForm.get('disabledFilter')?.disable();
    }
    else{
      this.selectedFiltersForm.get('featureToggleFilter')?.disable();
      this.selectedFiltersForm.get('featureToggleFilter')?.setValue(null);
      this.selectedFiltersForm.get('enabledFilter')?.enable();
      this.selectedFiltersForm.get('disabledFilter')?.enable();
    }
  }

  clearRtOptions(){
   if(this.selectedFiltersForm.value.featureToggleFilter){
    this.selectedFiltersForm.get('releaseToggleFilter')?.setValue(null);
    this.selectedFiltersForm.get('releaseToggleFilter')?.disable();
   }
   else{
    this.selectedFiltersForm.get('releaseToggleFilter')?.enable();
    this.selectedFiltersForm.get('releaseToggleFilter')?.setValue(null);
   }
  }

  removeFilters(){
    this.selectedFiltersForm.reset();
    this.selectedFiltersForm.get('releaseToggleFilter')?.enable();
    this.selectedFiltersForm.get('featureToggleFilter')?.enable();
  }
}
