import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ToasterService } from '@abp/ng.theme.shared';
import { HttpErrorReporterService } from '@abp/ng.core';


export const httpErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const toaster = inject(ToasterService);
  const httpErrorReporter = inject(HttpErrorReporterService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
     
      httpErrorReporter.reportError(error);

   
      if (error.status === 401) {
       
        toaster.error('Unauthorized', 'Please login to continue');
      } else if (error.status === 403) {
      
        toaster.error('Access Denied', 'You do not have permission to perform this action');
      } else if (error.status === 400) {
        
        const validationErrors = error.error?.error?.validationErrors;
        if (validationErrors && validationErrors.length > 0) {
        
          const firstError = validationErrors[0];
          toaster.error('Validation Error', firstError.message || 'Please check your input');
        } else {
          const errorMessage = error.error?.error?.message || error.error?.message || 'Invalid request';
          toaster.error('Error', errorMessage);
        }
      } else if (error.status === 404) {
       
        toaster.error('Not Found', 'The requested resource was not found');
      } else if (error.status === 500) {
        
        const errorMessage = error.error?.error?.message || 'An internal server error occurred';
        toaster.error('Server Error', errorMessage);
      } else if (error.status === 0) {
       
        toaster.error('Network Error', 'Unable to connect to the server. Please check your connection.');
      } else {
        
        const errorMessage = error.error?.error?.message || error.error?.message || 'An error occurred';
        toaster.error('Error', errorMessage);
      }

     
      return throwError(() => error);
    })
  );
};
