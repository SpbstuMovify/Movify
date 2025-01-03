package com.polytech.contentservice.service.episode;

import com.polytech.contentservice.dto.episode.EpisodeDto;
import com.polytech.contentservice.entity.Content;
import com.polytech.contentservice.entity.Episode;
import com.polytech.contentservice.exception.NotFoundException;
import com.polytech.contentservice.mapper.EpisodeMapper;
import com.polytech.contentservice.repository.ContentRepository;
import com.polytech.contentservice.repository.EpisodeRepository;
import jakarta.transaction.Transactional;
import java.util.UUID;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

/**
 * Реализация {@link EpisodeService}.
 */
@Service
@RequiredArgsConstructor
public class EpisodeServiceImpl implements EpisodeService {
    private final EpisodeRepository episodeRepository;
    private final ContentRepository contentRepository;
    private final EpisodeMapper episodeMapper;

    @Override
    public EpisodeDto createNewEpisode(UUID contentId, EpisodeDto episode) {
        Content content = contentRepository.findById(contentId)
            .orElseThrow(() -> new NotFoundException("Content not found"));
        Episode episodeToSave = episodeMapper.convertToEpisodeEntity(episode, content);
        return episodeMapper.convertToEpisodeDto(episodeRepository.save(episodeToSave));
    }

    @Override
    @Transactional
    public void updateEpisodeInfo(UUID episodeId, EpisodeDto episode) {
        Episode oldEpisode = episodeRepository.findById(episodeId)
                .orElseThrow(() -> new NotFoundException("Episode not found"));

        Episode updatedEpisode = episodeMapper.patchUpdate(oldEpisode, episode);
        episodeRepository.save(updatedEpisode);
    }

    @Override
    public EpisodeDto getEpisodeById(UUID id) {
        return episodeMapper.convertToEpisodeDto(episodeRepository.findById(id)
                .orElseThrow(() -> new NotFoundException("Episode not found")));
    }

    @Override
    @Transactional
    public void deleteEpisodeById(UUID id) {
        episodeRepository.findById(id)
            .orElseThrow(() -> new NotFoundException("Episode not found"));
        episodeRepository.deleteById(id);
    }
}
