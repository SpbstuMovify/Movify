package com.polytech.contentservice.common.converter;

import static org.junit.jupiter.api.Assertions.*;

import com.polytech.contentservice.common.AgeRestriction;
import org.junit.jupiter.api.extension.ExtendWith;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.EnumSource;
import org.junit.jupiter.params.provider.NullSource;
import org.junit.jupiter.params.provider.ValueSource;
import org.mockito.Spy;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class AgeRestrictionConverterTest {
  @Spy
  private AgeRestrictionConverter converter;

  @ParameterizedTest
  @EnumSource(AgeRestriction.class)
  void testConvertToDatabaseColumn(AgeRestriction ageRestriction) {
    assertEquals(ageRestriction.getRestriction(), converter.convertToDatabaseColumn(ageRestriction));
    assertNull(converter.convertToDatabaseColumn(null));
  }

  @ParameterizedTest
  @EnumSource(AgeRestriction.class)
  void testConvertToEntityAttribute(AgeRestriction ageRestriction) {
    assertEquals(ageRestriction, converter.convertToEntityAttribute(ageRestriction.getRestriction()));
    assertNull(converter.convertToEntityAttribute(null));
  }

  @ParameterizedTest
  @NullSource
  void testNullConvertToDatabaseColumn(AgeRestriction ageRestriction) {
    assertNull(converter.convertToDatabaseColumn(ageRestriction));
  }

  @ParameterizedTest
  @NullSource
  void testNullConvertToEntityAttribute(String ageRestriction) {
    assertNull(converter.convertToEntityAttribute(ageRestriction));
  }

  @ParameterizedTest
  @ValueSource(strings = { "12321", "pop", "1.5+", "+", " ", "" })
  void testConvertToEntityAttributeInvalid(String ageRestriction) {
    assertThrows(IllegalArgumentException.class, () -> converter.convertToEntityAttribute(ageRestriction));
  }

}
