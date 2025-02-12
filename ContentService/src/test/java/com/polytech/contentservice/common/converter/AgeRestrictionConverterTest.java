package com.polytech.contentservice.common.converter;

import static org.junit.jupiter.api.Assertions.*;

import com.polytech.contentservice.common.AgeRestriction;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Spy;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class AgeRestrictionConverterTest {
  @Spy
  private AgeRestrictionConverter converter;

  @Test
  void testConvertToDatabaseColumn() {
    assertEquals("6+", converter.convertToDatabaseColumn(AgeRestriction.SIX_PLUS));
    assertEquals("12+", converter.convertToDatabaseColumn(AgeRestriction.TWELVE_PLUS));
    assertEquals("16+", converter.convertToDatabaseColumn(AgeRestriction.SIXTEEN_PLUS));
    assertEquals("18+", converter.convertToDatabaseColumn(AgeRestriction.EIGHTEEN_PLUS));
    assertNull(converter.convertToDatabaseColumn(null));
  }

  @Test
  void testConvertToEntityAttribute() {
    assertEquals(AgeRestriction.SIX_PLUS, converter.convertToEntityAttribute("6+"));
    assertEquals(AgeRestriction.TWELVE_PLUS, converter.convertToEntityAttribute("12+"));
    assertEquals(AgeRestriction.SIXTEEN_PLUS, converter.convertToEntityAttribute("16+"));
    assertEquals(AgeRestriction.EIGHTEEN_PLUS, converter.convertToEntityAttribute("18+"));
    assertNull(converter.convertToEntityAttribute(null));
  }

  @Test
  void testConvertToEntityAttributeInvalid() {
    assertThrows(IllegalArgumentException.class, () -> converter.convertToEntityAttribute("21+"));
  }

}
