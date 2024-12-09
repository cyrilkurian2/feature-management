import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {
  isAdmin: boolean = false;


  constructor(private router: Router,
    private toastr: ToastrService,
    private authService: AuthService
  ) {

    this.isAdmin = this.authService.checkIsAdmin();
  }

  onLogout() {
    this.authService.deleteToken();
    this.router.navigate(['/user/login']);
    this.toastr.success('See you later!', 'Logout Successful');
  }
}
