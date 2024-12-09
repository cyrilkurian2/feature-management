import { Component, Inject, ViewChild } from '@angular/core';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatCommonModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelect, MatSelectModule } from '@angular/material/select';

import { FormControl, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { IBusiness } from '../interface/feature.interface';


@Component({
  selector: 'app-dialog',
  standalone: true,
  imports: [
    FormsModule,
    MatButtonModule,
    MatCommonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule,
    MatSelectModule,
  ],
  templateUrl: './dialog.component.html',
  styleUrl: './dialog.component.scss'
})



export class DialogComponent {
  @ViewChild('businessSelect') businessSelect!: MatSelect;
  businessControl = new FormControl<IBusiness | null>(null, Validators.required);
  searchControl = new FormControl(''); 
  filteredBusinesses: IBusiness[] = []; 

  constructor(
    public dialogRef: MatDialogRef<DialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { businesses: IBusiness[] }
  ) {
    this.filteredBusinesses = [...this.data.businesses];


  }

  filterBusinesses(): void {
    const searchTerm = this.searchControl.value?.toLowerCase() || '';
    console.log(searchTerm)

    this.filteredBusinesses = this.data.businesses.filter((business) =>
      business.businessName.toLowerCase().includes(searchTerm)
    );

    console.log(this.filteredBusinesses)
  }


  onSearchKeyUp(): void {
    const searchTerm = this.searchControl.value?.trim().toLowerCase() || '';
  
    if (searchTerm) {
      this.filterBusinesses();
      if (!this.businessSelect.panelOpen) {
        this.businessSelect.open();
      }
    } else {
      this.filteredBusinesses = [...this.data.businesses]; 
      if (this.businessSelect.panelOpen) {
        this.businessSelect.close();
      }
    }
  }
  
  onSelectionChange(): void {
    this.searchControl.setValue(''); 
    this.filteredBusinesses = [...this.data.businesses];
  }


  onCancel(): void {
    this.dialogRef.close();
  }

  onConfirm(): void {
    if (this.businessControl.valid) {
      this.dialogRef.close(this.businessControl.value);
    }
  }
}
