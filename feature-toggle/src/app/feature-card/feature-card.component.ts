import { NgClass } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DialogComponent } from '../dialog/dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { FeatureStatus, FeatureType } from '../enum/feature.enum';
import { IFeature, IBusiness, IUpdateToggle, IRetrievedFeatures, IselectedFilters, IPaginatedFeatures } from '../interface/feature.interface';
import { response } from 'express';

import { FeatureService } from '../feature.service';
import { ToastrService } from 'ngx-toastr';



@Component({
  selector: 'app-feature-card',
  standalone: true,
  imports: [NgClass, RouterModule],
  templateUrl: './feature-card.component.html',
  styleUrls: ['./feature-card.component.scss']
})

export class FeatureCardComponent {
  isAdmin: number = 0;
  currentUser: string | undefined;
  pageNumber: number = 0;

  constructor(
    public dialog: MatDialog,
    private featureService: FeatureService,
    private toastr: ToastrService
  ) {


    //take payload from jwt token
    const payload = this.featureService.decodeToken();

    payload.IsAdmin === "True" ? this.isAdmin = 1 : this.isAdmin = 0;

    this.currentUser = payload.UserID

    console.log("payload admin" + payload.IsAdmin)
    console.log("feature-card admin" + this.isAdmin)
    console.log("Current UserID: " + this.currentUser)
  }

  featureTypeEnum = FeatureType;
  featureStatusEnum = FeatureStatus;



  @Input() selectedFilters: IselectedFilters | null = null;
  paginatedfeatures: IPaginatedFeatures = {
    pageSize: 0,
    featureCount: 0,
    totalPages: 0,
    featureList: []
  }; 


  ngOnChanges() {
    if (this.selectedFilters) {
      this.pageNumber = 0
      this.fetchFeatures();
    }
  }

  fetchFeatures() {
    this.featureService.getFeatures(this.selectedFilters!,this.pageNumber).subscribe({
      next: (response) => {
        this.paginatedfeatures = response;
        console.log('Retrieved Features:', this.paginatedfeatures);
      },
      error: (err) => {
        console.error('Error fetching features:', err);
      },
    });
  }


  business: string | undefined;
  name: string | undefined;


  goToPage(page: number) {
    if (page >= 0 && page <= this.paginatedfeatures.totalPages) {
      this.pageNumber = page;
      this.fetchFeatures();
    }
  }

  nextPage() {
    if (this.pageNumber < this.paginatedfeatures.totalPages - 1) {
      this.pageNumber++;
      this.fetchFeatures();
    }
  }

  previousPage() {
    if (this.pageNumber > 0) {
      this.pageNumber--;
      this.fetchFeatures();
    }
  }


  openDialog(action: true | false, featureId: number): void {

    const apiEndpoint = action === true
      ? `/api/Business/Enable`
      : `/api/Business/Disable`;


    // Call the API to fetch businesses
    this.featureService.getBusinesses(apiEndpoint, featureId).subscribe({
      next: (response: IBusiness[]) => {
        // Open the dialog with the fetched businesses
        console.log(response);
        const dialogRef = this.dialog.open(DialogComponent, {
          width: '20%',
          data: {
            businesses: response
          }
        });

        // Handle dialog close
        dialogRef.afterClosed().subscribe((result: IBusiness | null) => {
          if (result) {
            this.business = result.businessId;
            console.log('Selected Business:', result);
            this.update_Toggle(featureId, Number(result.businessId), action); // Pass the businessId and action to update_Toggle
          }
        });
      },
      error: (error) => {
        console.error('Error fetching businesses:', error);
        alert('Failed to load businesses. Please try again.');
      }
    });
  }




  update_Toggle(featureId: number, businessId: number | null, featureStatus: boolean) {
    console.log(`Updating FeatureId: ${featureId}, BusinessId: ${businessId}, Status: ${featureStatus}, UserId: ${this.currentUser}`);

    const data: IUpdateToggle = {
      UserId: this.currentUser,
      featureId: featureId,
      businessId: businessId,
      enableOrDisable: featureStatus == true ? true : false

    }


    this.featureService.updateToggle(data).subscribe({
      next: (response: any) => {
        console.log(response)
        if (response === 1) {
          if (data.enableOrDisable == true) {

            this.toastr.success('Update Successful', 'Feature Enabled')
          }
          else {
            this.toastr.warning('Update Successful', 'Feature Disabled')
          }

        }   
        else{
          this.toastr.error('Update Unsuccessful', 'Something went wrong!')
        }   
      },
      error: (error) => {
        console.error('Error updating feature:', error);
        this.toastr.error('Update Unsuccessful', 'Something went wrong!')
      }
    });
  }


}
