import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../environments/environment';
import { IBusiness, IPaginatedFeatures, IPaginationLog, ISelectedFilters, IUpdateToggle } from '../interface/feature.interface';




@Injectable({
  providedIn: 'root'
})
export class FeatureService {
  private baseUrl = environment.apiUrl;
  public userId: number = 0;

  constructor(private router: Router, private http: HttpClient,private toastr: ToastrService) { }

  //for enabling or disabling feature
  updateToggle(data: IUpdateToggle): Observable<number> {
    return this.http.post<number>(`${this.baseUrl}/api/BusinessFeatureFlag/toggle`, data);
  }

  //for displaying business in dialog box 
  getBusinesses(action: boolean, featureId: number): Observable<IBusiness[]> {
    return this.http.get<IBusiness[]>(`${this.baseUrl}/api/Business?FeatureId=${featureId}&FeatureStatus=${action}`);
  }

  getLog(pageNumber: number, searchQuery: string): Observable<IPaginationLog> {
    const params = new HttpParams()
      .set('page', pageNumber)
      .set('pageSize', 12)
      .set('searchQuery', searchQuery !== null ? searchQuery : '')
    return this.http.get<IPaginationLog>(`${this.baseUrl}/api/Log`, { params })
  }


  getFeatures(selectedFilters2: ISelectedFilters, pageNumber: number): Observable<IPaginatedFeatures> {
    let params = new HttpParams()
    Object.entries(selectedFilters2).forEach(([key, value]) => {
    if (value !== null && value !== undefined) {
      params = params.set(key, value.toString());
    }
    });
    params = params.set('PageNumber', pageNumber)
    return this.http.get<IPaginatedFeatures>(`${this.baseUrl}/api/Feature`, { params });
  }

  downloadLogs() {
    return this.http.get(`${this.baseUrl}/api/Log/download-logs`, {
      responseType: 'blob', 
    });
  }
 
}
