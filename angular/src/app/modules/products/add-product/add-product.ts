import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ProductsService, CreateUpdateProductDto, ProductDto } from 'src/app/proxy/products';
import { CategoriesService, CategoryDto } from 'src/app/proxy/categories';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-add-product',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-product.html',
  styleUrl: './add-product.scss'
})
export class AddProduct implements OnInit {
  private productsService = inject(ProductsService);
  private categoriesService = inject(CategoriesService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private formBuilder = inject(FormBuilder);

  productForm: FormGroup;
  categories: CategoryDto[] = [];
  loading = false;
  saving = false;
  isEditMode = false;
  productId: number | null = null;
  activeCategories: CategoryDto[] = [];

  constructor() {
    this.buildForm();
  }

  ngOnInit(): void {
    this.loadCategories();

    // Check if we're in edit mode
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.productId = +params['id'];
        this.loadProduct(this.productId);
      }
    });
  }

  buildForm(): void {
    this.productForm = this.formBuilder.group({
      nameAr: ['', [Validators.required, Validators.maxLength(500)]],
      nameEn: ['', [Validators.required, Validators.maxLength(500)]],
      descriptionAr: ['', [Validators.required, Validators.maxLength(2000)]],
      descriptionEn: ['', [Validators.required, Validators.maxLength(2000)]],
      categoryId: [null, [Validators.required]],
      sku: ['', [Validators.required, Validators.maxLength(50), this.skuValidator]],
      price: [0, [Validators.required, Validators.min(0)]],
      stockQuantity: [0, [Validators.required, Validators.min(0)]],
      isActive: [true],
      isPublished: [false]
    });
  }

  skuValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    const skuPattern = /^[A-Z0-9-]+$/;
    if (!skuPattern.test(control.value)) {
      return { invalidSku: true };
    }
    return null;
  }

  loadCategories(): void {
    this.categoriesService.getActiveList().subscribe({
      next: (categories) => {
        this.activeCategories = categories;
        this.categories = categories;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
        // Fallback to all categories if active list fails
        this.categoriesService.getList({ maxResultCount: 1000, skipCount: 0 }).subscribe(res => {
          this.categories = res.items;
          this.activeCategories = res.items.filter(c => c.isActive);
        });
      }
    });
  }

  loadProduct(id: number): void {
    this.loading = true;
    this.productsService.get(id)
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (product: ProductDto) => {
          this.productForm.patchValue({
            nameAr: product.nameAr,
            nameEn: product.nameEn,
            descriptionAr: product.descriptionAr,
            descriptionEn: product.descriptionEn,
            categoryId: product.categoryId,
            sku: product.sku,
            price: product.price,
            stockQuantity: product.stockQuantity,
            isActive: product.isActive,
            isPublished: product.isPublished
          });
        },
        error: (error) => {
          console.error('Error loading product:', error);
          this.router.navigateByUrl('/products');
        }
      });
  }

  onSubmit(): void {
    if (this.productForm.invalid) {
      this.markFormGroupTouched(this.productForm);
      return;
    }

    this.saving = true;
    const formValue = this.productForm.value;

    const categoryId = formValue.categoryId != null
      ? (typeof formValue.categoryId === 'string' ? parseInt(formValue.categoryId, 10) : Number(formValue.categoryId))
      : 0;

    const productDto: CreateUpdateProductDto = {
      id: this.isEditMode ? this.productId! : 0,
      nameAr: formValue.nameAr || '',
      nameEn: formValue.nameEn || '',
      descriptionAr: formValue.descriptionAr || '',
      descriptionEn: formValue.descriptionEn || '',
      categoryId: categoryId,
      sku: (formValue.sku || '').toUpperCase().trim(),
      price: formValue.price != null ? (typeof formValue.price === 'string' ? parseFloat(formValue.price) : Number(formValue.price)) : 0,
      stockQuantity: formValue.stockQuantity != null ? (typeof formValue.stockQuantity === 'string' ? parseInt(formValue.stockQuantity, 10) : Number(formValue.stockQuantity)) : 0,
      isActive: Boolean(formValue.isActive),
      isPublished: Boolean(formValue.isActive ? formValue.isPublished : false)
    };

    const operation = this.isEditMode
      ? this.productsService.update(this.productId!, productDto)
      : this.productsService.create(productDto);

    operation
      .pipe(finalize(() => this.saving = false))
      .subscribe({
        next: () => {
          this.router.navigateByUrl('/products');
        },
        error: (error) => {
          console.error('Error saving product:', error);


          if (error?.error?.error?.validationErrors) {
            const validationErrors = error.error.error.validationErrors;

            // Apply validation errors to form controls
            validationErrors.forEach((validationError: any) => {
              const fieldName = validationError.members?.[0] || '';
              const errorMessage = validationError.message || '';

              if (fieldName) {
                const control = this.productForm.get(fieldName);
                if (control) {
                  // Set custom error on the control
                  control.setErrors({
                    serverError: true,
                    serverErrorMessage: errorMessage
                  });
                  control.markAsTouched();
                }
              }
            });

            // Mark form as touched to show errors
            this.markFormGroupTouched(this.productForm);
          }
        }
      });
  }

  cancel(): void {
    this.router.navigateByUrl('/products');
  }

  markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  getFieldError(fieldName: string): string {
    const field = this.productForm.get(fieldName);

    if (field?.hasError('serverError') && field.errors?.['serverErrorMessage']) {
      return field.errors['serverErrorMessage'];
    }

    if (field?.hasError('required')) {
      return `${this.getFieldLabel(fieldName)} is required`;
    }
    if (field?.hasError('maxlength')) {
      const maxLength = field.errors?.['maxlength']?.requiredLength;
      return `${this.getFieldLabel(fieldName)} must not exceed ${maxLength} characters`;
    }
    if (field?.hasError('min')) {
      return `${this.getFieldLabel(fieldName)} must be greater than or equal to 0`;
    }
    if (field?.hasError('invalidSku')) {
      return 'SKU must contain only uppercase letters, numbers, and hyphens';
    }
    return '';
  }

  getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      nameAr: 'Arabic Name',
      nameEn: 'English Name',
      descriptionAr: 'Arabic Description',
      descriptionEn: 'English Description',
      categoryId: 'Category',
      sku: 'SKU',
      price: 'Price',
      stockQuantity: 'Stock Quantity'
    };
    return labels[fieldName] || fieldName;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.productForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  onIsActiveChange(): void {
    if (!this.productForm.get('isActive')?.value) {
      this.productForm.patchValue({ isPublished: false });
    }
  }
}
