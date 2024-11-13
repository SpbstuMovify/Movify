package com.polytech.contentservice.conroller;

import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequiredArgsConstructor
@RequestMapping("/v1/contents")
public class ContentController {
  @GetMapping
  public void test() {
    System.out.println("gg");
  }
}
