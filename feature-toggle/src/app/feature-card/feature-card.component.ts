import { NgClass } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DialogComponent } from '../dialog/dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { FeatureStatus, FeatureType } from '../enum/feature.enum';
import { IBusiness, IUpdateToggle, ISelectedFilters, IPaginatedFeatures } from '../interface/feature.interface';

import { ToastrService } from 'ngx-toastr';
import { FeatureService } from '../services/feature.service';
import { AuthService } from '../services/auth.service';



@Component({
  selector: 'app-feature-card',
  standalone: true,
  imports: [NgClass, RouterModule],
  templateUrl: './feature-card.component.html',
  styleUrls: ['./feature-card.component.scss']
})

export class FeatureCardComponent {
  isAdmin: boolean;
  currentUser: string | undefined;
  pageNumber: number = 0;
  business: IBusiness | undefined;  // for displaying bussiness id in dialog
  isLoading: boolean = true;

  constructor(
    public dialog: MatDialog,
    private featureService: FeatureService,
    private authService: AuthService,
    private toastr: ToastrService
  ) {

    //payload from jwt token
    this.isAdmin = this.authService.checkIsAdmin();
    this.currentUser = this.authService.getUserId();

  }

  featureTypeEnum = FeatureType;
  featureStatusEnum = FeatureStatus;

  @Input() selectedFilters : ISelectedFilters | null = null

  paginatedfeatures: IPaginatedFeatures = {
    pageSize: 0,
    featureCount: 0,
    totalPages: 0,
    featureList: []
  };


  ngOnChanges() {
     if (this.selectedFilters) {
      this.isLoading = true;
      this.pageNumber = 0;
      this.fetchFeatures();
    }
  }

  fetchFeatures() {
    this.featureService.getFeatures(this.selectedFilters!, this.pageNumber).subscribe({
      next: (response) => {
        this.paginatedfeatures = response;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching features:', err);
      },
    });
  }


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
    // Call the API to fetch businesses
    this.featureService.getBusinesses(action, featureId).subscribe({
      next: (response: IBusiness[]) => {
        // Open the dialog with the fetched businesses
        const dialogRef = this.dialog.open(DialogComponent, {
          width: '20%',
          data: {
            businesses: response
          }
        });

        // Handle dialog close
        dialogRef.afterClosed().subscribe((result: IBusiness | null) => {
          if (result) {
            // this.business = result;
            this.updateFeatureToggle(featureId, Number(result.businessId), action);
          }
        });
      },
      error: (error) => {
        console.error('Error fetching businesses:', error);
        alert('Failed to load businesses. Please try again.');
      }
    });
  }


  updateFeatureToggle(featureId: number, businessId: number | null, featureStatus: boolean) {

    const data: IUpdateToggle = {
      UserId: this.currentUser,
      featureId: featureId,
      businessId: businessId,
      enableOrDisable: featureStatus == true ? true : false

    }


    this.featureService.updateToggle(data).subscribe({
      next: (response: number) => {
        if (response === 1) {
          if (data.enableOrDisable == true) {

            this.toastr.success('Update Successful', 'Feature Enabled')
          }
          else {
            this.toastr.warning('Update Successful', 'Feature Disabled')
          }

        }
        else {
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
