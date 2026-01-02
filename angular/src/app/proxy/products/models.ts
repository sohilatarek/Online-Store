import type { EntityDto, FullAuditedEntityDto } from '@abp/ng.core';

export interface CreateUpdateProductDto extends EntityDto<number> {
  nameAr?: string;
  nameEn?: string;
  descriptionAr?: string;
  descriptionEn?: string;
  categoryId: number;
  extraDescription?: string;
}

export interface ProductDto extends FullAuditedEntityDto<number> {
  nameAr?: string;
  nameEn?: string;
  descriptionAr?: string;
  descriptionEn?: string;
  categoryId: number;
  categoryNameAr?: string;
  categoryNameEn?: string;
  categoryDescriptionAr?: string;
  categoryDescriptionEn?: string;
}
