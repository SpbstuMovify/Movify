package com.polytech.contentservice.common.converter;

import static org.junit.jupiter.api.Assertions.*;

import com.polytech.contentservice.common.Quality;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Spy;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class QualityConverterTest {

  @Spy
  private QualityConverter converter;

  @Test
  void testConvertToDatabaseColumn() {
    assertEquals("720P", converter.convertToDatabaseColumn(Quality.P7200));
    assertEquals("1080P", converter.convertToDatabaseColumn(Quality.P1080));
    assertNull(converter.convertToDatabaseColumn(null));
  }

  @Test
  void testConvertToEntityAttribute() {
    assertEquals(Quality.P7200, converter.convertToEntityAttribute("720P"));
    assertEquals(Quality.P1080, converter.convertToEntityAttribute("1080P"));
    assertNull(converter.convertToEntityAttribute(null));
  }

  @Test
  void testConvertToEntityAttributeInvalid() {
    assertThrows(IllegalArgumentException.class, () -> converter.convertToEntityAttribute("999P"));
  }
}
