import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ILoginAccept, ILoginForm, ILoginReturn } from '../../interface/feature.interface';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../services/auth.service';



@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  userForm: FormGroup<ILoginForm>;
  isSubmitted: boolean = false;
  isLoading?: boolean ;

  constructor(private fb: FormBuilder,
    private router: Router,
    private authService: AuthService,
    private toastr: ToastrService
  ) {
    this.userForm = this.fb.group({
      email: new FormControl(
        '',
        [Validators.required, Validators.pattern(/^[a-zA-Z0-9._%+-]+@geekywolf\.com$/)]
      ),
      password: new FormControl('', Validators.required)
    });
  }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/home']);
    }
  }

  onSubmit() {
    this.isLoading = true;
    if (this.userForm.valid) {
      const userDetails: ILoginAccept = {
        email: this.userForm.value.email ?? '',
        password: this.userForm.value.password ?? ''
      }

      this.authService.login(userDetails).subscribe({
        next: (response: ILoginReturn) => {

          if (response.token !== null) {
            this.authService.saveToken(response.token);
            this.authService.checkIsAdmin();
            this.router.navigate(['/home']);
            this.toastr.success('Welcome back!', 'Login Successful');
            this.isLoading = false;
          }
          else {
            this.toastr.error('Something went wrong', 'Login failed')
            this.isLoading = false;
            throw new Error('Login failed');
          }
        },
        error: (error) => {
          if (error.status === 400) {
            this.toastr.error('Invalid login credentials', 'Login failed')
          }
          else {
            this.toastr.error('Something went wrong', 'Login failed')
          }
        }
      });
    }
    else {
      this.toastr.error('Something went wrong', 'Login failed')
    }
  }

  hasDisplayableError(controlName: string): boolean {
    const control = this.userForm.get(controlName);
    return !!control?.invalid && (this.isSubmitted || control?.touched)

  }
}




