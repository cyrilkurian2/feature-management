import { HttpHandlerFn, HttpInterceptorFn, HttpRequest, HttpResponse } from '@angular/common/http';
import { TOKEN_KEY } from './shared/constants';
import { ToastrService } from 'ngx-toastr';
import { inject } from '@angular/core';
import { tap } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn) => {
  const authToken = localStorage.getItem(TOKEN_KEY);
  const toastr = inject(ToastrService);


  if (!authToken) {
    return next(req);
  }

  const newRq = req.clone({
    headers: req.headers.set('Authorization', `Bearer ${authToken}`),
  });

  return next(newRq).pipe(
    tap({
      next: (event) => {
        if (event instanceof HttpResponse && (event.status === 500 || event.status === 401 || event.status === 400)) {
          toastr.error('Try again later','Something went wrong');
        }
      },
    })
  );

};
