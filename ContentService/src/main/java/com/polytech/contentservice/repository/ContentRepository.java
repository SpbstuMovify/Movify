package com.polytech.contentservice.repository;

import com.polytech.contentservice.entity.Content;
import com.polytech.contentservice.entity.QContent;
import com.querydsl.core.types.dsl.StringExpression;
import java.util.UUID;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.querydsl.QuerydslPredicateExecutor;
import org.springframework.data.querydsl.binding.QuerydslBinderCustomizer;
import org.springframework.data.querydsl.binding.QuerydslBindings;

/**
 * Взаимодействие с таблицей content.
 */
public interface ContentRepository extends JpaRepository<Content, UUID>,
    QuerydslPredicateExecutor<Content>, QuerydslBinderCustomizer<QContent> {
  @Override
  default void customize(QuerydslBindings bindings, QContent product) {
    bindings.bind(product.title).first(StringExpression::containsIgnoreCase);
  }
}
