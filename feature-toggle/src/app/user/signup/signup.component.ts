import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ValidatorFn, ReactiveFormsModule } from '@angular/forms';
import { ISignUpAccept, ISignUpForm, ISignUpReturn } from '../../interface/feature.interface';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../services/auth.service';


@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss'
})
export class SignupComponent implements OnInit {
  userForm: FormGroup<ISignUpForm>;
  isLoading?: boolean ;

  passwordMatchValidator: ValidatorFn = (control: AbstractControl): null => {
    const password = control.get('password')
    const confirmPassword = control.get('confirmPassword')

    if (password && confirmPassword && password.value != confirmPassword.value)
      confirmPassword?.setErrors({ passwordMismatch: true })
    else
      confirmPassword?.setErrors(null)

    return null;
  }

  constructor(private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {

    this.userForm = this.fb.group({
      fullName: new FormControl('', Validators.required),
      email: new FormControl('', [
        Validators.required, Validators.email, Validators.pattern(/^[a-zA-Z0-9._%+-]+@geekywolf\.com$/)
      ]),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(6),
        Validators.pattern(/(?=.*[^a-zA-Z0-9])/)
      ]),
      confirmPassword: new FormControl('')
      ,
    }, { validators: this.passwordMatchValidator })
  }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/home']);
    }
  }

  isSubmitted: boolean = false;


  onSubmit() {
    this.isLoading = true;
    this.isSubmitted = true;
    if (this.userForm.valid) {
      const userData: ISignUpAccept = {
        name: this.userForm.value.fullName ?? '',
        email: this.userForm.value.email ?? '',
        password: this.userForm.value.password ?? '',
      }


      this.authService.addUser(userData).subscribe({
        next: (response: ISignUpReturn) => {
          if (response.success) {
            this.toastr.success('New user created!', 'Registration Successful');
            this.router.navigate(['user/login']);
            this.isLoading = false;
          }
          else {
            this.toastr.error('User was not created', 'Registration Unsuccessful');
            this.isLoading = false;
          }

        },
        error: (error) => {

          this.toastr.error('User was not created', 'Registration Unsuccessful')

        }
      });
    } else {

      this.toastr.error('Enter valid details', 'Registration Unsuccessful')
    }
  }

  hasDisplayableError(controlName: string): Boolean {
    const control = this.userForm.get(controlName);
    return Boolean(control?.invalid) && (this.isSubmitted || Boolean(control?.touched))

  }
}
