<?php

namespace App\Controller;

use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\Finder\Finder;
use Symfony\Component\HttpFoundation\JsonResponse;
use Symfony\Component\HttpFoundation\Request;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Annotation\Route;
use Symfony\Contracts\Cache\CacheInterface;
use Symfony\Contracts\Cache\ItemInterface;

class ApiController extends AbstractController
{
    #[Route('/api/cmd/{name}', name: 'api.cmd', requirements: ['name' => 'komi'])]
    public function additionalCommand(
        Request $request,
        string $name,
        CacheInterface $cache
    ): JsonResponse {
        $cache->delete('api.cmd.'.$name);
        $files = $cache->get('api.cmd.'.$name, function (ItemInterface $item) use ($name, $request) {
            $item->expiresAfter(60 * 60 * 24);
            $finder = new Finder();

            $items = [];

            foreach ($finder->in(__DIR__.'/../../public/additional-cmds/'.$name)->files() as $file) {
                $items[] = $request->getSchemeAndHttpHost().'/additional-cmds/'.$name.'/'.$file->getFilename();
            }

            return $items;
        });

        if (empty($files)) {
            return $this->json([], Response::HTTP_NOT_FOUND);
        }

        return $this->json([
            'url' => $files[random_int(0, count($files) - 1)]
        ]);
    }
}
