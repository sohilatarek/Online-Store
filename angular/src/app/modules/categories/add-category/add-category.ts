import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CategoriesService, CreateUpdateCategoryDto, CategoryDto } from 'src/app/proxy/categories';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-add-category',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-category.html',
  styleUrl: './add-category.scss'
})
export class AddCategory implements OnInit {
  private categoriesService = inject(CategoriesService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private formBuilder = inject(FormBuilder);

  categoryForm: FormGroup;
  loading = false;
  saving = false;
  isEditMode = false;
  categoryId: number | null = null;

  constructor() {
    this.buildForm();
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.categoryId = +params['id'];
        this.loadCategory(this.categoryId);
      }
    });
  }

  buildForm(): void {
    this.categoryForm = this.formBuilder.group({
      nameAr: ['', [Validators.required, Validators.maxLength(500)]],
      nameEn: ['', [Validators.required, Validators.maxLength(500)]],
      descriptionAr: ['', [Validators.required, Validators.maxLength(2000)]],
      descriptionEn: ['', [Validators.required, Validators.maxLength(2000)]],
      isActive: [true],
      displayOrder: [0, [Validators.required, Validators.min(0)]]
    });
  }

  loadCategory(id: number): void {
    this.loading = true;
    this.categoriesService.get(id)
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: (category: CategoryDto) => {
          this.categoryForm.patchValue({
            nameAr: category.nameAr,
            nameEn: category.nameEn,
            descriptionAr: category.descriptionAr,
            descriptionEn: category.descriptionEn,
            isActive: category.isActive,
            displayOrder: category.displayOrder
          });
        },
        error: (error) => {
          console.error('Error loading category:', error);
          this.router.navigateByUrl('/categories');
        }
      });
  }

  onSubmit(): void {
    if (this.categoryForm.invalid) {
      this.markFormGroupTouched(this.categoryForm);
      return;
    }

    this.saving = true;
    const formValue = this.categoryForm.value;
    const categoryDto: CreateUpdateCategoryDto = {
      id: this.isEditMode ? this.categoryId! : 0,
      nameAr: formValue.nameAr,
      nameEn: formValue.nameEn,
      descriptionAr: formValue.descriptionAr,
      descriptionEn: formValue.descriptionEn,
      isActive: formValue.isActive,
      displayOrder: formValue.displayOrder
    };

    const operation = this.isEditMode
      ? this.categoriesService.update(this.categoryId!, categoryDto)
      : this.categoriesService.create(categoryDto);

    operation
      .pipe(finalize(() => this.saving = false))
      .subscribe({
        next: () => {
          this.router.navigateByUrl('/categories');
        },
        error: (error) => {
          console.error('Error saving category:', error);
        }
      });
  }

  cancel(): void {
    this.router.navigateByUrl('/categories');
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
    const field = this.categoryForm.get(fieldName);
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
    return '';
  }

  getFieldLabel(fieldName: string): string {
    const labels: { [key: string]: string } = {
      nameAr: 'Arabic Name',
      nameEn: 'English Name',
      descriptionAr: 'Arabic Description',
      descriptionEn: 'English Description',
      displayOrder: 'Display Order'
    };
    return labels[fieldName] || fieldName;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.categoryForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }
}
